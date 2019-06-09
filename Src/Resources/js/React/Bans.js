import React, { Component } from 'react';
import { render } from 'react-dom';
import { BanRow, ManageInfo } from './Components/BansChildren';

const baseUri = "/admin/bans";

export default class BanManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            bans: null,
            banIndex: null,
        }
    }

    componentDidMount() {
        this.getData();
    }

    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/active`,
        }).done((activeBans) => {
            this.setState({
                bans: activeBans,
                banIndex: null
            });
        }).fail((err) => {
            console.error(`React/Bans {Bans@getData} - Error getting active bans`, err.responseText);
        })
    }

    getBans() {
        if(!!this.state.bans)
            return this.state.bans;

        return null;
    }

    setBanIndex(index) {
        this.setState({ banIndex: index});
    }

    submitForm(e) {
        e.preventDefault();

        let uri = `${baseUri}`;
        let method = "post";

        if (this.state.banIndex != null) {
            uri = `${baseUri}/${this.state.bans[this.state.banIndex].id}`;
            method = "put";
        }

        $.ajax({
            type: method,
            url: uri,
            data: {
                name: $("input#lookup_account").val(),
                expires_at: $("input#banExpires").val(),
                reason: $("textarea#banReason").val()
            }
        }).done(() => {
            this.getData();
            this.setBanIndex(null);
        }).fail((err) => {
            console.error(`React/Bans {Bans@submitForm} - Error saving or updating ban`, err.responseText);
        });

    }

    revokeBan(banId) {
        $.ajax({
            type: 'delete',
            url: `${baseUri}/${banId}`
        }).done(() => {
            this.getData();
        }).fail((err) => {
            console.error(`React/Bans {Bans@revokeBan} - Error revoking ban id: ${banId}`, err.responseText);
        });
    }

    render() {
        let bans;
        if (!!this.getBans()) {
            bans = this.getBans().map((ban, index) => {
                return <BanRow ban={ban} viewDetails={this.setBanIndex.bind(this)} revokeBan={this.revokeBan.bind(this)} index={index} key={index} />
            });
        }

        let banDetails = <ManageInfo onSubmit={this.submitForm.bind(this)} reset={this.setBanIndex.bind(this)}/>;
        if (this.state.bans != null && this.state.banIndex != null) {
            banDetails = <ManageInfo details={this.state.bans[this.state.banIndex]} onSubmit={this.submitForm.bind(this)} reset={this.setBanIndex.bind(this)}/>
        }

        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-8 col-md-12">
                        <table className="table table-responsive">
                            <thead>
                                <tr className="font-alpha">
                                    <th></th>
                                    <th>Name</th>
                                    <th>Admin</th>
                                    <th></th>
                                    <th></th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {bans} 
                            </tbody>
                        </table>
                    </div>

                    <div className="col-lg-4 col-md-12">
                        {banDetails}
                    </div>
                </div>
            </div>
        );
    }
}

if (document.getElementById('bans'))
    render(<BanManagement />, document.getElementById('bans'));