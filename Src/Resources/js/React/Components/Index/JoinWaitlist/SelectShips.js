import React, { Component } from 'react';

export default class SelectShips extends Component {
    updateFits(e) {
        this.props.selectedFit(e.target.checked, Number(e.target.name));
    }

    render() {
        let fits;
        if (this.props.fits && this.props.fits.length > 0) {
            fits = this.props.fits.map((fit) => {
                return (
                    <div className="col-12 pb-3">
                        <label className="custom-control custom-checkbox">
                            <input type="checkbox" className="custom-control-input" onChange={this.updateFits.bind(this)} name={fit.id} />
                            <span className="custom-control-indicator"></span>
                            <span className="custom-control-description">
                                <img src={`https://image.eveonline.com/Render/${fit.typeId}_32.png`} /> {fit.description}
                            </span>
                        </label>
                    </div>
                )
            });
        } else {
            fits = (
                <p className="text-center">
                    <a className="xmpp" href="/account-settings">Add a Fit Here</a>
                </p>
            )
        }

        return (
            <div className="row  pb-4">
                <div className="col-12">
                    <label className="pb-3">What ships do you wish to fly?</label>
                    {fits}
                </div>
            </div>
        )
    }
}