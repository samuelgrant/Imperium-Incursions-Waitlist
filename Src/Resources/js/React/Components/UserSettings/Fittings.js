import React, { Component } from 'react';
import { TextArea } from '../FormControls';
import Modal from '../Modal';

export default class Fittings extends Component {
    constructor(props) {
        super(props);

        this.state = {
            key: 0,
        }
    }

    getFits() {
        return this.props.fits || null;
    }

    saveFit(e) {
        e.preventDefault();

        $.ajax({
            type: 'post',
            url: `${this.props.baseUri}/fit`,
            data: {fitUrl: e.target[0].value}
        }).done(() => {
            this.setState({ key: this.state.key + 1 });
            this.props.forceUpdate();
        }).fail((err) => {
            console.error(`React/Components/FittingsManagement {FittingsManagement@submitNewFit} - Error saving a new fit`, err.responseText);
        })
    }

    deleteFit(id) {
        $.ajax({
            type: 'delete',
            url: `${this.props.baseUri}/fit/${id}`
        }).done(() => {
            this.props.forceUpdate();
        }).fail((err) => {
            console.error(`React/Components/FittingsManagement {FittingsManagement@deleteFit} - Error deleting fit (Fit Id: ${id})`, err.responseText);
        })
    }

    render() {
        let newFit;
        if (this.getFits() && this.getFits().length < 5) {
            newFit = <SubmitFit saveFit={this.saveFit.bind(this)} key={this.state.key}/>
        }

        let activeFits;
        if (this.getFits()) {
            activeFits = this.getFits().map((fit) => {
                return <FitRow fit={fit} deleteFit={this.deleteFit.bind(this, fit.id)} />
            });
        }

        return (
            <div>
                <h3>My Fits</h3>
                <p className="ml-4 mb-0">You will be asked to select up to four ships from the list below, when you join the waitlist.</p>
                <p className="ml-4">You can have up to 5 ships stored below at any given time.</p>

                <div className="table-responsive table-hover">
                    <table className="table">
                        <thead>
                            <tr>
                                <th></th>
                                <th>Hull Type</th>
                                <th>Description</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            {activeFits}
                        </tbody>
                    </table>

                    {newFit}
                </div>

                <FittingsHelpModal />
            </div>
        )
    }
}

export class FitRow extends Component {
    render() {
        return (
            <tr>
                <td><img src={`https://image.eveonline.com/Render/${this.props.fit.shipTypeId}_32.png`} /></td>
                <td>{this.props.fit.name}</td>
                <td>{this.props.fit.description}</td>
                <td><button class="btn btn-danger btn-sm" type="button" onClick={this.props.deleteFit}>Delete Fit <i class="fas fa-trash"></i></button></td>
            </tr>
        )
    }
}

export class FittingsHelpModal extends Component {
    render() {
        return (
            <Modal id="fittingsHelp" size="lg" dismiss="true" title="How to save a fit.">
                <p>To add a fitting you will need your Fit DNA URL. The name of your fit will be saved as the fit description.</p>
                <ol>
                    <li>Drag and drop your fit into a chat channel to make a clickable link and press enter</li>
                    <li>Right click next to the link and click copy</li>
                    <li>Paste it in the text box and click 'Save Fitting'</li>
                </ol>

                <div class="embed-responsive embed-responsive-16by9">
                    <video autoplay="true" loop="true">
                        <source className="embed-responsive-item" src="/images/savefit.mp4" type="video/mp4"/>
                    </video>
                </div>
            </Modal>
        );
    }
}

export class SubmitFit extends Component {
    render() {
        return (
            <form id="newFittingForm" className="form-group" onSubmit={this.props.saveFit}>
                <label htmlFor="fitDna">Add a new ship</label>
                <div className="row">
                    <div className="col-9">
                        <TextArea id="fitDna" required="true" placeholder="[00:25:25] Caitlin Viliana > <url=fitting:17740:26448;1:26402;1:15144;4:3186;8:14512;1:41201;1:33842;2:26322;1:14650;2:4347;2::>Vindicator</url>" key={this.props.key} />
                        <a className="float-right pt-2" data-toggle="modal" data-target="#fittingsHelp"> How do I add a new fit?</a>
                    </div>
                    <div className="col-3 text-center">
                        <button className="btn btn-success d-block mb-2" type="submit">Save Fitting</button>
                    </div>
                </div>
            </form>
        )
    }
}